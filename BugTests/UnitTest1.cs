using BugPro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BugTests;

[TestClass]
public class BugTests
{
    private Bug _bug = null!;

    [TestInitialize]
    public void Setup()
    {
        _bug = new Bug();
    }

    [TestMethod]
    public void Bug_InitialState_ShouldBeNew()
    {
        Assert.AreEqual(Bug.State.New, _bug.CurrentState);
    }

    [TestMethod]
    public void Assign_FromNew_ShouldChangeToAssigned()
    {
        _bug.Assign("Tester1");
        Assert.AreEqual(Bug.State.Assigned, _bug.CurrentState);
    }

    [TestMethod]
    public void Reject_FromNew_ShouldChangeToRejected()
    {
        _bug.Reject();
        Assert.AreEqual(Bug.State.Rejected, _bug.CurrentState);
    }

    [TestMethod]
    public void Defer_FromNew_ShouldChangeToDeferred()
    {
        _bug.Defer();
        Assert.AreEqual(Bug.State.Deferred, _bug.CurrentState);
    }

    [TestMethod]
    public void StartProgress_FromAssigned_ShouldChangeToInProgress()
    {
        _bug.Assign("Dev1");
        _bug.StartProgress();
        Assert.AreEqual(Bug.State.InProgress, _bug.CurrentState);
    }

    [TestMethod]
    public void Fix_FromInProgress_ShouldChangeToFixed()
    {
        _bug.Assign("Dev1");
        _bug.StartProgress();
        _bug.Fix();
        Assert.AreEqual(Bug.State.Fixed, _bug.CurrentState);
    }

    [TestMethod]
    public void Verify_FromFixed_ShouldChangeToVerified()
    {
        _bug.Assign("Dev1");
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        Assert.AreEqual(Bug.State.Verified, _bug.CurrentState);
    }

    [TestMethod]
    public void Close_FromVerified_ShouldChangeToClosed()
    {
        _bug.Assign("Dev1");
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        _bug.Close();
        Assert.AreEqual(Bug.State.Closed, _bug.CurrentState);
    }

    [TestMethod]
    public void Reopen_FromClosed_ShouldChangeToReopened()
    {
        _bug.Assign("Dev1");
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        _bug.Close();
        _bug.Reopen();
        Assert.AreEqual(Bug.State.Reopened, _bug.CurrentState);
    }

    [TestMethod]
    public void Reject_FromAssigned_ShouldChangeToRejected()
    {
        _bug.Assign("Dev1");
        _bug.Reject();
        Assert.AreEqual(Bug.State.Rejected, _bug.CurrentState);
    }

    [TestMethod]
    public void Reactivate_FromRejected_ShouldChangeToNew()
    {
        _bug.Reject();
        _bug.Reactivate();
        Assert.AreEqual(Bug.State.New, _bug.CurrentState);
    }

    [TestMethod]
    public void Reactivate_FromDeferred_ShouldChangeToNew()
    {
        _bug.Defer();
        _bug.Reactivate();
        Assert.AreEqual(Bug.State.New, _bug.CurrentState);
    }

    [TestMethod]
    public void Reopen_FromFixed_ShouldChangeToReopened()
    {
        _bug.Assign("Dev1");
        _bug.StartProgress();
        _bug.Fix();
        _bug.Reopen();
        Assert.AreEqual(Bug.State.Reopened, _bug.CurrentState);
    }

    [TestMethod]
    public void Reopen_FromVerified_ShouldChangeToReopened()
    {
        _bug.Assign("Dev1");
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        _bug.Reopen();
        Assert.AreEqual(Bug.State.Reopened, _bug.CurrentState);
    }

    [TestMethod]
    public void Reject_FromReopened_ShouldChangeToRejected()
    {
        _bug.Reject();
        _bug.Reactivate();
        _bug.Assign("Dev1");
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        _bug.Close();
        _bug.Reopen();
        _bug.Reject();
        Assert.AreEqual(Bug.State.Rejected, _bug.CurrentState);
    }

    [TestMethod]
    public void FullHappyPath_ShouldEndInClosed()
    {
        _bug.Assign("Tester");
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        _bug.Close();
        Assert.AreEqual(Bug.State.Closed, _bug.CurrentState);
    }

    [TestMethod]
    public void StartProgress_FromNew_ShouldThrowException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _bug.StartProgress());
    }

    [TestMethod]
    public void Fix_FromAssigned_ShouldThrowException()
    {
        _bug.Assign("Dev1");
        Assert.ThrowsException<InvalidOperationException>(() => _bug.Fix());
    }

    [TestMethod]
    public void Verify_FromInProgress_ShouldThrowException()
    {
        _bug.Assign("Dev1");
        _bug.StartProgress();
        Assert.ThrowsException<InvalidOperationException>(() => _bug.Verify());
    }

    [TestMethod]
    public void Close_FromFixed_ShouldThrowException()
    {
        _bug.Assign("Dev1");
        _bug.StartProgress();
        _bug.Fix();
        Assert.ThrowsException<InvalidOperationException>(() => _bug.Close());
    }

    [TestMethod]
    public void Reject_FromClosed_ShouldThrowException()
    {
        _bug.Assign("Dev1");
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        _bug.Close();
        Assert.ThrowsException<InvalidOperationException>(() => _bug.Reject());
    }

    [TestMethod]
    public void Reopen_FromNew_ShouldThrowException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _bug.Reopen());
    }

    [TestMethod]
    public void Close_FromReopened_ShouldThrowException()
    {
        _bug.Assign("Dev1");
        _bug.StartProgress();
        _bug.Fix();
        _bug.Reopen();
        Assert.ThrowsException<InvalidOperationException>(() => _bug.Close());
    }
}
