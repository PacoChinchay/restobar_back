using System.Net.Sockets;
using System.Text;
using restobar_core.Application.DTOs;

namespace restobar_core.Application.Services;

public class KitchenPrinterService(IConfiguration config, ILogger<KitchenPrinterService> logger)
{
    private static readonly TimeZoneInfo Lima =
        TimeZoneInfo.FindSystemTimeZoneById("America/Lima");

    private const int LineWidth = 42;

    // ── ESC/POS command bytes ────────────────────────────────────────────────
    private static readonly byte[] Init       = [0x1B, 0x40];
    private static readonly byte[] CodePage   = [0x1B, 0x74, 0x10]; // WPC1252 – includes á é í ó ú ñ
    private static readonly byte[] AlignLeft  = [0x1B, 0x61, 0x00];
    private static readonly byte[] AlignCenter= [0x1B, 0x61, 0x01];
    private static readonly byte[] BoldOn     = [0x1B, 0x45, 0x01];
    private static readonly byte[] BoldOff    = [0x1B, 0x45, 0x00];
    private static readonly byte[] PartialCut = [0x1D, 0x56, 0x01];
    private static readonly byte[] LF         = [0x0A];

    public async Task PrintKitchenTicketAsync(OrderDto order, List<OrderItemInput> allItems)
    {
        if (!bool.Parse(config["KitchenPrinter:Enabled"] ?? "true")) return;

        var kitchenItems = allItems
            .Where(i => !string.Equals(i.MenuType, "drinks", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (kitchenItems.Count == 0) return;

        var bytes = BuildTicket(order, kitchenItems);

        var host    = config["KitchenPrinter:Host"]!;
        var port    = int.Parse(config["KitchenPrinter:Port"]!);
        var timeout = int.Parse(config["KitchenPrinter:TimeoutMs"] ?? "3000");

        try
        {
            using var client = new TcpClient { SendTimeout = timeout, ReceiveTimeout = timeout };
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout));
            await client.ConnectAsync(host, port, cts.Token);
            await using var stream = client.GetStream();
            await stream.WriteAsync(bytes, cts.Token);
            await stream.FlushAsync(cts.Token);
            logger.LogInformation("Kitchen ticket sent: Mesa {Table}, {Count} item(s)",
                order.TableNumber, kitchenItems.Count);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Kitchen printer {Host}:{Port} unreachable — {Msg}", host, port, ex.Message);
        }
    }

    // ── Ticket builder ───────────────────────────────────────────────────────
    private static byte[] BuildTicket(OrderDto order, List<OrderItemInput> items)
    {
        var buf = new List<byte>(512);

        void Bytes(byte[] b)   => buf.AddRange(b);
        void Text(string s)    => buf.AddRange(Encoding.Latin1.GetBytes(s));
        void NewLine()         => buf.AddRange(LF);
        void Line(string s)    { Text(s); NewLine(); }
        void Separator()       => Line(new string('-', LineWidth));
        void BlankLine()       => NewLine();

        var printTime = TimeZoneInfo.ConvertTimeFromUtc(order.CreatedAt.UtcDateTime, Lima);

        // ── Init + code page ────────────────────────────────────────────────
        Bytes(Init);
        Bytes(CodePage);

        // ── Header ──────────────────────────────────────────────────────────
        Bytes(AlignCenter);
        Bytes(BoldOn);
        Line(Center($"COMANDA  Mesa {order.TableNumber}"));
        Bytes(BoldOff);
        Line(Center(printTime.ToString("dd/MM/yyyy  HH:mm")));
        if (!string.IsNullOrEmpty(order.CreatedBy))
            Line(Center($"Mesero: {order.CreatedBy}"));
        Bytes(AlignLeft);
        Separator();

        // ── Items grouped: Carta first, then Menu del dia, then others ───────
        var groups = new[]
        {
            ("CARTA",        items.Where(i => i.MenuType == "food").ToList()),
            ("MENU DEL DIA", items.Where(i => i.MenuType == "daily").ToList()),
            ("OTROS",        items.Where(i => i.MenuType is null or "").ToList()),
        };

        foreach (var (label, group) in groups)
        {
            if (group.Count == 0) continue;
            Bytes(BoldOn);
            Line(label);
            Bytes(BoldOff);
            Separator();
            foreach (var item in group)
                Line(FormatItemLine(item.ProductName, item.Quantity, item.UnitPrice));
            Separator();
        }

        BlankLine();
        BlankLine();
        BlankLine();
        Bytes(PartialCut);

        return [.. buf];
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    // Centers text within LineWidth
    private static string Center(string text)
    {
        if (text.Length >= LineWidth) return text[..LineWidth];
        var pad = LineWidth - text.Length;
        return text.PadLeft(text.Length + pad / 2).PadRight(LineWidth);
    }

    // Formats one item line fitting exactly LineWidth characters
    // Example at 42 chars:
    //   Lomo Saltado Peruano           x 2  S/34.00
    //   Ceviche Mixto                  x 1  S/50.00
    private static string FormatItemLine(string name, int qty, decimal unitPrice)
    {
        var total    = unitPrice * qty;
        var right    = $"  x {qty}  S/{total:F2}"; // " x 2  S/34.00"
        var nameWidth = LineWidth - right.Length;
        if (nameWidth < 1) nameWidth = 1;

        var truncName = name.Length > nameWidth
            ? name[..nameWidth]
            : name.PadRight(nameWidth);

        return truncName + right;
    }
}
