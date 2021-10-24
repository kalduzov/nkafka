namespace NKafka.Clients.Admin;

public record CreateTopicsOptions(int TimeoutMs, bool ValidateOnly, bool RetryOnQuotaViolation): AdminOptions(TimeoutMs);