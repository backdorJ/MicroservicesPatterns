namespace Contracts.Events;

public class Envelope
{
    public string MessageType { get; set; }
    public string Payload { get; set; }
}