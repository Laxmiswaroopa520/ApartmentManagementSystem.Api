using System;

public class OtpService : IOtpService
{
    public string GenerateOtp()
    {
        return new Random().Next(100000, 999999).ToString();
    }

    public Task SendOtpAsync(string mobile, string otp)
    {
        Console.WriteLine($"OTP sent to {mobile}: {otp}");
        return Task.CompletedTask;
    }
}

