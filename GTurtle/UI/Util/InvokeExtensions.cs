using System;
using System.Windows.Forms;

namespace GTurtle
{
    public static class InvokeExtensions
    {

        //Utility function to invoke on an action [on a Windows Forms Control], instead of a delegate.
        public static void InvokeAction(this Control ctrl, Action action)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(action);
            }
            else
            {
                action();
            }
        }

        //Utility function to invoke on a value-returning function, instead of a delegate.
        public static T InvokeFunc<T>(this Control ctrl, Func<T> func)
        {
            T ret_val = default(T);
            ctrl.InvokeAction(() => { ret_val = func(); });
            return ret_val;
        }
    }
}
