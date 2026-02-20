namespace CIDemo;

public class PaymentGateway
{
    private readonly Random _random = new();

    public async Task<string> ProcessPaymentAsync(string transactionId, decimal amount)
    {
        if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("Invalid transaction");
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");

        // Simulate network latency
        await Task.Delay(_random.Next(50, 200));

        // Simulate 40% failure rate (Flaky dependency)
        // MTP Intelligent Retry should handle this in CI
        if (_random.NextDouble() < 0.4)
        {
            throw new HttpRequestException($"Gateway timeout for transaction {transactionId}");
        }

        return $"SUCCESS:{transactionId}:{amount}";
    }

    public async Task<PaymentStatus> GetPaymentStatusAsync(string transactionId)
    {
        if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("Invalid transaction");

        await Task.Delay(_random.Next(20, 100));

        // Simulate flaky status check (30% failure rate)
        if (_random.NextDouble() < 0.3)
        {
            return PaymentStatus.Unknown;
        }

        return PaymentStatus.Completed;
    }

    public decimal CalculateFee(decimal amount, string currency)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

        return currency.ToUpperInvariant() switch
        {
            "EUR" => amount * 0.015m,
            "USD" => amount * 0.020m,
            "GBP" => amount * 0.018m,
            _ => amount * 0.030m
        };
    }

    public bool ValidateCard(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber)) return false;

        // Luhn algorithm
        var digits = cardNumber.Where(char.IsDigit).Select(c => c - '0').ToArray();
        if (digits.Length < 13 || digits.Length > 19) return false;

        int sum = 0;
        bool alternate = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int n = digits[i];
            if (alternate)
            {
                n *= 2;
                if (n > 9) n -= 9;
            }
            sum += n;
            alternate = !alternate;
        }
        return sum % 10 == 0;
    }

    // Metodo per refund — non coperto dai test (coverage < 100%)
    public async Task<string> RefundPaymentAsync(string transactionId, decimal amount)
    {
        if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("Invalid transaction");
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

        await Task.Delay(_random.Next(100, 300));

        if (_random.NextDouble() < 0.2)
        {
            throw new InvalidOperationException($"Refund failed for {transactionId}: service unavailable");
        }

        return $"REFUND:{transactionId}:{amount}";
    }

    // Metodo per batch — non coperto dai test (coverage < 100%)
    public async Task<IReadOnlyList<string>> ProcessBatchAsync(IEnumerable<(string Id, decimal Amount)> payments)
    {
        var results = new List<string>();
        foreach (var (id, amount) in payments)
        {
            var result = await ProcessPaymentAsync(id, amount);
            results.Add(result);
        }
        return results;
    }
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Unknown
}
