using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CIDemo;

[TestClass]
public class PaymentGatewayTests
{
    private PaymentGateway _gateway = null!;

    [TestInitialize]
    public void Setup()
    {
        _gateway = new PaymentGateway();
    }

    // ── Flaky tests: richiedono --retry-failed-tests in CI ──

    [TestMethod]
    [TestCategory("Flaky")]
    public async Task ProcessPayment_ShouldSucceed_Eventually()
    {
        // FLAKY by design (40% failure rate).
        // Relies on MTP '--retry-failed-tests' to pass in CI.
        var result = await _gateway.ProcessPaymentAsync(Guid.NewGuid().ToString(), 99.99m);

        Assert.IsTrue(result.StartsWith("SUCCESS"), $"Expected SUCCESS but got: {result}");
    }

    [TestMethod]
    [TestCategory("Flaky")]
    public async Task ProcessPayment_LargeAmount_ShouldSucceed_Eventually()
    {
        // Flaky: single payment with 40% failure rate
        var transactionId = Guid.NewGuid().ToString();
        var result = await _gateway.ProcessPaymentAsync(transactionId, 1500.00m);

        Assert.IsTrue(result.Contains(transactionId));
        Assert.IsTrue(result.Contains("1500"));
    }

    [TestMethod]
    [TestCategory("Flaky")]
    public async Task GetPaymentStatus_ShouldReturnCompleted_Eventually()
    {
        // Flaky: 30% chance of returning Unknown
        var status = await _gateway.GetPaymentStatusAsync("TX-001");

        Assert.AreEqual(PaymentStatus.Completed, status);
    }

    // ── Stable tests: validation & business logic ──

    [TestMethod]
    public async Task ProcessPayment_ShouldThrow_WhenTransactionIdIsEmpty()
    {
        await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
        {
            await _gateway.ProcessPaymentAsync("", 100m);
        });
    }

    [TestMethod]
    public async Task ProcessPayment_ShouldThrow_WhenTransactionIdIsNull()
    {
        await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
        {
            await _gateway.ProcessPaymentAsync(null!, 100m);
        });
    }

    [TestMethod]
    public async Task ProcessPayment_ShouldThrow_WhenAmountIsZero()
    {
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
        {
            await _gateway.ProcessPaymentAsync("TX-001", 0m);
        });
    }

    [TestMethod]
    public async Task ProcessPayment_ShouldThrow_WhenAmountIsNegative()
    {
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
        {
            await _gateway.ProcessPaymentAsync("TX-001", -50m);
        });
    }

    [TestMethod]
    [TestCategory("Flaky")]
    public async Task ProcessPayment_ShouldReturnCorrectFormat()
    {
        var transactionId = "TX-12345";
        var amount = 99.99m;

        var result = await _gateway.ProcessPaymentAsync(transactionId, amount);

        StringAssert.StartsWith(result, "SUCCESS:");
        StringAssert.Contains(result, transactionId);
        StringAssert.Contains(result, amount.ToString());
    }

    // ── GetPaymentStatus tests ──

    [TestMethod]
    public async Task GetPaymentStatus_ShouldThrow_WhenTransactionIdIsEmpty()
    {
        await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
        {
            await _gateway.GetPaymentStatusAsync("");
        });
    }

    // ── CalculateFee tests ──

    [TestMethod]
    public void CalculateFee_EUR_ShouldApply1Point5Percent()
    {
        var fee = _gateway.CalculateFee(100m, "EUR");
        Assert.AreEqual(1.50m, fee);
    }

    [TestMethod]
    public void CalculateFee_USD_ShouldApply2Percent()
    {
        var fee = _gateway.CalculateFee(100m, "USD");
        Assert.AreEqual(2.00m, fee);
    }

    [TestMethod]
    public void CalculateFee_GBP_ShouldApply1Point8Percent()
    {
        var fee = _gateway.CalculateFee(100m, "GBP");
        Assert.AreEqual(1.80m, fee);
    }

    [TestMethod]
    public void CalculateFee_OtherCurrency_ShouldApply3Percent()
    {
        var fee = _gateway.CalculateFee(100m, "JPY");
        Assert.AreEqual(3.00m, fee);
    }

    [TestMethod]
    public void CalculateFee_ShouldThrow_WhenAmountIsZero()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
        {
            _gateway.CalculateFee(0m, "EUR");
        });
    }

    // ── ValidateCard tests (Luhn algorithm) ──

    [TestMethod]
    public void ValidateCard_ValidVisa_ShouldReturnTrue()
    {
        Assert.IsTrue(_gateway.ValidateCard("4111111111111111"));
    }

    [TestMethod]
    public void ValidateCard_ValidMastercard_ShouldReturnTrue()
    {
        Assert.IsTrue(_gateway.ValidateCard("5500000000000004"));
    }

    [TestMethod]
    public void ValidateCard_InvalidNumber_ShouldReturnFalse()
    {
        Assert.IsFalse(_gateway.ValidateCard("1234567890123456"));
    }

    [TestMethod]
    public void ValidateCard_TooShort_ShouldReturnFalse()
    {
        Assert.IsFalse(_gateway.ValidateCard("411111"));
    }

    [TestMethod]
    public void ValidateCard_Empty_ShouldReturnFalse()
    {
        Assert.IsFalse(_gateway.ValidateCard(""));
    }

    [TestMethod]
    public void ValidateCard_Null_ShouldReturnFalse()
    {
        Assert.IsFalse(_gateway.ValidateCard(null!));
    }

    // ── Sanity check ──

    [TestMethod]
    [TestCategory("Stable")]
    public void PaymentGateway_ShouldBeInstantiable()
    {
        var gateway = new PaymentGateway();
        Assert.IsNotNull(gateway);
    }
}
