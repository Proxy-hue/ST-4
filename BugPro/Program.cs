using Stateless;

namespace BugPro;

public class Bug
{
    public enum State
    {
        New,
        Assigned,
        InProgress,
        Fixed,
        Verified,
        Closed,
        Reopened,
        Rejected,
        Deferred
    }

    public enum Trigger
    {
        Assign,
        StartProgress,
        Fix,
        Verify,
        Close,
        Reopen,
        Reject,
        Defer,
        Reactivate
    }

    private readonly StateMachine<State, Trigger> _machine;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _assignTrigger;

    public Bug(State initialState = State.New)
    {
        _machine = new StateMachine<State, Trigger>(initialState);
        _assignTrigger = _machine.SetTriggerParameters<string>(Trigger.Assign);

        _machine.Configure(State.New)
            .Permit(Trigger.Assign, State.Assigned)
            .Permit(Trigger.Reject, State.Rejected)
            .Permit(Trigger.Defer, State.Deferred);

        _machine.Configure(State.Assigned)
            .OnEntryFrom(_assignTrigger, developer => Console.WriteLine($"  Assigned developer: {developer}"))
            .Permit(Trigger.StartProgress, State.InProgress)
            .Permit(Trigger.Reject, State.Rejected)
            .Permit(Trigger.Defer, State.Deferred);

        _machine.Configure(State.InProgress)
            .Permit(Trigger.Fix, State.Fixed)
            .Permit(Trigger.Defer, State.Deferred);

        _machine.Configure(State.Fixed)
            .Permit(Trigger.Verify, State.Verified)
            .Permit(Trigger.Reopen, State.Reopened);

        _machine.Configure(State.Verified)
            .Permit(Trigger.Close, State.Closed)
            .Permit(Trigger.Reopen, State.Reopened);

        _machine.Configure(State.Closed)
            .Permit(Trigger.Reopen, State.Reopened);

        _machine.Configure(State.Reopened)
            .Permit(Trigger.Assign, State.Assigned)
            .Permit(Trigger.Reject, State.Rejected);

        _machine.Configure(State.Rejected)
            .Permit(Trigger.Reactivate, State.New);

        _machine.Configure(State.Deferred)
            .Permit(Trigger.Reactivate, State.New);
    }

    public State CurrentState => _machine.State;

    public void Assign(string developer) => _machine.Fire(_assignTrigger, developer);
    public void StartProgress() => _machine.Fire(Trigger.StartProgress);
    public void Fix() => _machine.Fire(Trigger.Fix);
    public void Verify() => _machine.Fire(Trigger.Verify);
    public void Close() => _machine.Fire(Trigger.Close);
    public void Reopen() => _machine.Fire(Trigger.Reopen);
    public void Reject() => _machine.Fire(Trigger.Reject);
    public void Defer() => _machine.Fire(Trigger.Defer);
    public void Reactivate() => _machine.Fire(Trigger.Reactivate);

    public static void Main()
    {
        Console.WriteLine("Bug workflow demo");

        var bug = new Bug();
        Console.WriteLine($"Initial state: {bug.CurrentState}");

        bug.Assign("Yushkova Polina");
        Console.WriteLine($"After assign: {bug.CurrentState}");

        bug.StartProgress();
        Console.WriteLine($"After start progress: {bug.CurrentState}");

        bug.Fix();
        Console.WriteLine($"After fix: {bug.CurrentState}");

        bug.Verify();
        Console.WriteLine($"After verify: {bug.CurrentState}");

        bug.Close();
        Console.WriteLine($"After close: {bug.CurrentState}");
    }
}
