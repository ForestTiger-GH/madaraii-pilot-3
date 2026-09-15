namespace MiniDoc;

internal readonly record struct OpenTransitionTicket(long Intent, long ContentRevision);

internal sealed class OpenTransitionGuard
{
    private long _intent;
    private long _contentRevision;

    internal long BeginOpenIntent() => ++_intent;

    internal void SupersedeOpen() => _intent++;

    internal OpenTransitionTicket Capture(long intent) => new(intent, _contentRevision);

    internal void ContentChanged() => _contentRevision++;

    internal bool IsCurrentIntent(long intent) => intent == _intent;

    internal bool IsCurrent(OpenTransitionTicket ticket) =>
        ticket.Intent == _intent && ticket.ContentRevision == _contentRevision;
}
