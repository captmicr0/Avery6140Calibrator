using System;
using System.Windows.Input;

namespace AD.API.SampleApplication.Commands;

public class Command : ICommand
{
	protected Action action;

	protected Action<object> parameterizedAction;

	private bool canExecute;

	public bool CanExecute
	{
		get
		{
			return canExecute;
		}
		set
		{
			if (canExecute != value)
			{
				canExecute = value;
				this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public event EventHandler CanExecuteChanged;

	public Command(Action action, bool canExecute = true)
	{
		this.action = action;
		this.canExecute = canExecute;
	}

	public Command(Action<object> parameterizedAction, bool canExecute = true)
	{
		this.parameterizedAction = parameterizedAction;
		this.canExecute = canExecute;
	}

	bool ICommand.CanExecute(object parameter)
	{
		return canExecute;
	}

	void ICommand.Execute(object parameter)
	{
		DoExecute(parameter);
	}

	public void Execute(object parameter)
	{
		DoExecute(parameter);
	}

	protected void InvokeAction(object param)
	{
		Action theAction = action;
		Action<object> theParameterizedAction = parameterizedAction;
		if (theAction != null)
		{
			theAction();
		}
		else
		{
			theParameterizedAction?.Invoke(param);
		}
	}

	public virtual void DoExecute(object param)
	{
		InvokeAction(param);
	}
}
