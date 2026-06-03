using Stateless;

namespace BugPro;

public class Bug
{
    private const string EmptyAssignee = "(not specified)";

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
    private string _assignee = EmptyAssignee;

    public Bug(State initialState = State.New)
    {
        _machine = new StateMachine<State, Trigger>(initialState);
        _assignTrigger = _machine.SetTriggerParameters<string>(Trigger.Assign);

        ConfigureIntakeStates();
        ConfigureDevelopmentStates();
        ConfigureReviewStates();
        ConfigureFinalStates();
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

    private void ConfigureIntakeStates()
    {
        _machine.Configure(State.New)
            .Permit(Trigger.Assign, State.Assigned)
            .Permit(Trigger.Reject, State.Rejected)
            .Permit(Trigger.Defer, State.Deferred);

        _machine.Configure(State.Assigned)
            .OnEntryFrom(_assignTrigger, RememberAssignee)
            .Permit(Trigger.StartProgress, State.InProgress)
            .Permit(Trigger.Reject, State.Rejected)
            .Permit(Trigger.Defer, State.Deferred);
    }

    private void ConfigureDevelopmentStates()
    {
        _machine.Configure(State.InProgress)
            .Permit(Trigger.Fix, State.Fixed)
            .Permit(Trigger.Defer, State.Deferred);

        _machine.Configure(State.Fixed)
            .Permit(Trigger.Verify, State.Verified)
            .Permit(Trigger.Reopen, State.Reopened);
    }

    private void ConfigureReviewStates()
    {
        _machine.Configure(State.Verified)
            .Permit(Trigger.Close, State.Closed)
            .Permit(Trigger.Reopen, State.Reopened);

        _machine.Configure(State.Closed)
            .Permit(Trigger.Reopen, State.Reopened);

        _machine.Configure(State.Reopened)
            .Permit(Trigger.Assign, State.Assigned)
            .Permit(Trigger.Reject, State.Rejected);
    }

    private void ConfigureFinalStates()
    {
        _machine.Configure(State.Rejected)
            .Permit(Trigger.Reactivate, State.New);

        _machine.Configure(State.Deferred)
            .Permit(Trigger.Reactivate, State.New);
    }

    private void RememberAssignee(string developer)
    {
        _assignee = string.IsNullOrWhiteSpace(developer) ? EmptyAssignee : developer.Trim();
        Console.WriteLine($"  Responsible engineer: {_assignee}");
    }

    public static void Main()
    {
        Console.WriteLine("Defect workflow sample");

        var bug = new Bug();
        PrintStep("created", bug);

        bug.Assign("Yushkova Polina");
        PrintStep("sent to developer", bug);

        bug.StartProgress();
        PrintStep("work started", bug);

        bug.Fix();
        PrintStep("fix prepared", bug);

        bug.Verify();
        PrintStep("qa accepted", bug);

        bug.Close();
        PrintStep("closed", bug);
    }

    private static void PrintStep(string label, Bug bug)
    {
        Console.WriteLine($"{label,-18} -> {bug.CurrentState} ({bug._assignee})");
    }
}
