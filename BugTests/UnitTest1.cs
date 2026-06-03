using BugPro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BugTests;

[TestClass]
public class BugTests
{
    private Bug _bug = null!;

    [TestInitialize]
    public void CreateFreshBug()
    {
        _bug = new Bug();
    }

    [TestMethod]
    public void CreatedBug_StaysInNewQueue()
    {
        AssertState(Bug.State.New);
    }

    [TestMethod]
    public void Assign_NewBug_MovesToAssigned()
    {
        AssignToDeveloper();

        AssertState(Bug.State.Assigned);
    }

    [TestMethod]
    public void Reject_NewBug_SendsItToRejectedBucket()
    {
        _bug.Reject();

        AssertState(Bug.State.Rejected);
    }

    [TestMethod]
    public void Defer_NewBug_PutsItOnHold()
    {
        _bug.Defer();

        AssertState(Bug.State.Deferred);
    }

    [TestMethod]
    public void StartProgress_AssignedBug_OpensDevelopment()
    {
        AssignToDeveloper();
        _bug.StartProgress();

        AssertState(Bug.State.InProgress);
    }

    [TestMethod]
    public void Fix_InProgressBug_WaitsForVerification()
    {
        MoveToDevelopment();
        _bug.Fix();

        AssertState(Bug.State.Fixed);
    }

    [TestMethod]
    public void Verify_FixedBug_ConfirmsTesterCheck()
    {
        MoveToFixed();
        _bug.Verify();

        AssertState(Bug.State.Verified);
    }

    [TestMethod]
    public void Close_VerifiedBug_FinishesWorkflow()
    {
        MoveToVerified();
        _bug.Close();

        AssertState(Bug.State.Closed);
    }

    [TestMethod]
    public void Reopen_ClosedBug_ReturnsToReopenedState()
    {
        MoveToClosed();
        _bug.Reopen();

        AssertState(Bug.State.Reopened);
    }

    [TestMethod]
    public void Reject_AssignedBug_EndsAsRejected()
    {
        AssignToDeveloper();
        _bug.Reject();

        AssertState(Bug.State.Rejected);
    }

    [TestMethod]
    public void Reactivate_RejectedBug_RestartsFromNew()
    {
        _bug.Reject();
        _bug.Reactivate();

        AssertState(Bug.State.New);
    }

    [TestMethod]
    public void Reactivate_DeferredBug_RestartsFromNew()
    {
        _bug.Defer();
        _bug.Reactivate();

        AssertState(Bug.State.New);
    }

    [TestMethod]
    public void Reopen_FixedBug_RoutesBackForWork()
    {
        MoveToFixed();
        _bug.Reopen();

        AssertState(Bug.State.Reopened);
    }

    [TestMethod]
    public void Reopen_VerifiedBug_RoutesBackForWork()
    {
        MoveToVerified();
        _bug.Reopen();

        AssertState(Bug.State.Reopened);
    }

    [TestMethod]
    public void Assign_ReopenedBug_ReturnsToAssigned()
    {
        MoveToClosed();
        _bug.Reopen();
        _bug.Assign("Anton");

        AssertState(Bug.State.Assigned);
    }

    [TestMethod]
    public void Reject_ReopenedBug_ClosesAsRejected()
    {
        MoveToClosed();
        _bug.Reopen();
        _bug.Reject();

        AssertState(Bug.State.Rejected);
    }

    [TestMethod]
    public void MainPositiveScenario_EndsInClosedState()
    {
        MoveToClosed();

        AssertState(Bug.State.Closed);
    }

    [TestMethod]
    public void Defer_AssignedBug_PausesBeforeDevelopment()
    {
        AssignToDeveloper();
        _bug.Defer();

        AssertState(Bug.State.Deferred);
    }

    [TestMethod]
    public void Defer_InProgressBug_PausesActiveWork()
    {
        MoveToDevelopment();
        _bug.Defer();

        AssertState(Bug.State.Deferred);
    }

    [TestMethod]
    public void StartProgress_NewBug_IsRejectedByStateless()
    {
        AssertInvalidTransition(() => _bug.StartProgress());
    }

    [TestMethod]
    public void Fix_AssignedBug_IsRejectedByStateless()
    {
        AssignToDeveloper();

        AssertInvalidTransition(() => _bug.Fix());
    }

    [TestMethod]
    public void Verify_InProgressBug_IsRejectedByStateless()
    {
        MoveToDevelopment();

        AssertInvalidTransition(() => _bug.Verify());
    }

    [TestMethod]
    public void Close_FixedBug_IsRejectedByStateless()
    {
        MoveToFixed();

        AssertInvalidTransition(() => _bug.Close());
    }

    [TestMethod]
    public void Reject_ClosedBug_IsRejectedByStateless()
    {
        MoveToClosed();

        AssertInvalidTransition(() => _bug.Reject());
    }

    [TestMethod]
    public void Reopen_NewBug_IsRejectedByStateless()
    {
        AssertInvalidTransition(() => _bug.Reopen());
    }

    [TestMethod]
    public void Close_ReopenedBug_IsRejectedByStateless()
    {
        MoveToFixed();
        _bug.Reopen();

        AssertInvalidTransition(() => _bug.Close());
    }

    private void AssignToDeveloper()
    {
        _bug.Assign("Ivan");
    }

    private void MoveToDevelopment()
    {
        AssignToDeveloper();
        _bug.StartProgress();
    }

    private void MoveToFixed()
    {
        MoveToDevelopment();
        _bug.Fix();
    }

    private void MoveToVerified()
    {
        MoveToFixed();
        _bug.Verify();
    }

    private void MoveToClosed()
    {
        MoveToVerified();
        _bug.Close();
    }

    private void AssertState(Bug.State expected)
    {
        Assert.AreEqual(expected, _bug.CurrentState);
    }

    private static void AssertInvalidTransition(Action action)
    {
        Assert.ThrowsException<InvalidOperationException>(action);
    }
}
