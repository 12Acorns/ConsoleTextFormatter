using NEG.CTF2.Core.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NEG.CTF2.Core.Diagnostics;

public sealed class DiagnosticResults
{
	public DiagnosticResults(DiagnosticResults _results)
	{
		if(!_results.CanAdd)
		{
			throw new Exception($"Can not create as {_results.results} as span is already in use");
		}

		HasError = _results.HasError;
		results = _results.results;
		resultQueue = [];
	}
	public DiagnosticResults(IEnumerable<DiagnosticResult> _results)
	{
		if(!_results.Any())
		{
			results = [];
			resultQueue = [];
			return;
		}

		results = [.._results];
		resultQueue = [];
		HasError = true;
	}
	public DiagnosticResults()
	{
		results = [];
		resultQueue = [];
	}

	public bool HasError { get; private set; } = false;
	public bool CanAdd { get; private set; } = true;

	internal readonly List<DiagnosticResult> results;
	internal readonly List<DiagnosticResult> resultQueue;

	internal Type? currentUser = null;

	/// <param name="_result"></param>
	/// <returns>
	/// True: Added result to internal list
	/// <para></para>
	/// False: Could not add to internal list as already in use by another user, will add
	/// to an internal queue in which once can add the queued items will be added.
	/// </returns>
	public bool Add(DiagnosticResult _result)
	{
		HasError = true;

		if(CanAdd)
		{
			results.Add(_result);
			return true;
		}
		resultQueue.Add(_result);
		return false;
	}
	/// <summary>
	/// Get a <see cref="ReadOnlySpan{DiagnosticResult}"/> of the internal list
	/// </summary>
	/// <param name="_result">The results</param>
	/// <returns>
	/// True: Could return the span as no other users
	/// <para></para>
	/// False: Could not return the span as already in use
	/// </returns>
	public bool GetResults<T>(out ReadOnlySpan<DiagnosticResult> _result)
	{
		if(!CanAdd)
		{
			_result = [];
			return false;
		}

		CanAdd = false;
		_result = CollectionsMarshal.AsSpan(results);
		currentUser = typeof(T);
		return true;
	}
	/// <summary>
	/// Will allow other users to access internal array and adds queued items to 
	/// main results
	/// </summary>
	/// <returns>
	/// True: User who is accessing span freed access for other users
	/// <para></para>
	/// False: User who does not have access tried to free for self or other users, 
	/// did not release as result
	/// </returns>
	public bool FinishedWithResults<T>()
	{
		if(currentUser is not null && typeof(T) != currentUser)
		{
			return false;
		}

		CanAdd = true;
		currentUser = null;
		if(resultQueue.Count == 0)
		{
			return true;
		}

		resultQueue.ClearTo(results);
		return true;
	}
	public override string ToString()
	{
		if(results.Count == 0)
		{
			if(resultQueue.Count > 0)
			{
				return "No (non-queued) errors";
			}
			return "No errors";
		}

		var _builder = new StringBuilder();
		foreach(var _error in results)
		{
			_builder.AppendLine(_error.ToString());
		}
		return _builder.ToString();
	}
}
