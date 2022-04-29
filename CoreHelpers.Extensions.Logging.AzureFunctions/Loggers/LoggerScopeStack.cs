using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using CoreHelpers.Extensions.Logging.Abstractions;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.Loggers
{    
    internal class LoggerScopeStack
    {
        private static AsyncLocal<LoggerScopeStack> _value = new AsyncLocal<LoggerScopeStack>();

        private LoggerScopeStack(IReadOnlyDictionary<string, object> state, LoggerScopeStack parent)
        {
            State = state;
            Parent = parent;
        }

        internal IReadOnlyDictionary<string, object> State { get; private set; }

        internal LoggerScopeStack Parent { get; private set; }

        public static LoggerScopeStack Current
        {
            get
            {
                return _value.Value;
            }

            set
            {
                _value.Value = value;
            }
        }

        public static IDisposable Push<TState>(TState state, IScopedLogger assiocatedLogger)
        {
            IDictionary<string, object> stateValues;

            if (state is IEnumerable<KeyValuePair<string, object>> stateEnum)
            {
                // Convert this to a dictionary as we have scenarios where we cannot have duplicates. In this
                // case, if there are dupes, the later entry wins.
                stateValues = new Dictionary<string, object>();
                foreach (var entry in stateEnum)
                {
                    stateValues[entry.Key] = entry.Value;
                }
            }
            else
            {
                // There's nothing we can do with other states.
                return null;
            }

            Current = new LoggerScopeStack(new ReadOnlyDictionary<string, object>(stateValues), Current);
            return new DisposableScope<TState>(state, assiocatedLogger);
        }

        // Builds a state dictionary of all scopes. If an inner scope
        // contains the same key as an outer scope, it overwrites the value.
        public static IDictionary<string, object> GetMergedStateDictionaryOrNull()
        {
            IDictionary<string, object> scopeInfo = null;

            var current = Current;
            while (current != null)
            {
                if (scopeInfo == null)
                {
                    scopeInfo = new Dictionary<string, object>();
                }

                foreach (var entry in current.State)
                {
                    // inner scopes win
                    if (!scopeInfo.Keys.Contains(entry.Key))
                    {
                        scopeInfo.Add(entry);
                    }
                }
                current = current.Parent;
            }

            return scopeInfo;
        }

        private class DisposableScope<TState> : IDisposable
        {
            private IScopedLogger _assiocatedLogger;
            private TState _state;

            public DisposableScope(TState state, IScopedLogger assiocatedLogger)
            {
                _assiocatedLogger = assiocatedLogger;
                _state = state;
            }

            public void Dispose()
            {
                if (_assiocatedLogger != null)
                    _assiocatedLogger.DisposeScope<TState>(_state);

                Current = Current.Parent;
            }
        }
    }
}
