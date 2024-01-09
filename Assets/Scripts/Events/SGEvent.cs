using System;

namespace SG
{
    public class SGEvent
    {
        private event Action action;

        public void RegisterEvent(Action action)
        {
            this.action += action;
        }

        public void UnRegisterEvent(Action action)
        {
            this.action -= action;
        }

        public void TriggerEvent()
        {
            action?.Invoke();
        }
    }
    
    public class SGEvent<T0, T1>
    {
        private event Action<T0, T1> action;

        public void RegisterEvent(Action<T0, T1> action)
        {
            this.action += action;
        }

        public void UnRegisterEvent(Action<T0, T1> action)
        {
            this.action -= action;
        }

        public void TriggerEvent(T0 arg0, T1 arg1)
        {
            action?.Invoke(arg0, arg1);
        }
    }
    
    public class SGEvent<T0, T1, T2>
    {
        private event Action<T0, T1, T2> action;

        public void RegisterEvent(Action<T0, T1, T2> action)
        {
            this.action += action;
        }

        public void UnRegisterEvent(Action<T0, T1, T2> action)
        {
            this.action -= action;
        }

        public void TriggerEvent(T0 arg0, T1 arg1, T2 arg2)
        {
            action?.Invoke(arg0, arg1, arg2);
        }
    }
}
