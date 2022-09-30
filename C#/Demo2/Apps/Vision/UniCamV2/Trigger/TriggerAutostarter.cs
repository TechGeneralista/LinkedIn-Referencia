using Common;
using System;
using System.Windows.Controls;
using UVC.Internals;

namespace UniCamV2.Trigger
{
    public class TriggerAutoStarter
    {
        readonly TriggerDC triggerDC;
        bool saveIsRun;
        int saveDelay;


        public TriggerAutoStarter(TriggerDC triggerDC) => this.triggerDC = triggerDC;


        internal void Start()
        {
            saveIsRun = triggerDC.IsRun;
            saveDelay = triggerDC.Delay.Value;

            if(!saveIsRun)
            {
                triggerDC.Delay.Value = triggerDC.MinimumDelay;
                triggerDC.IsInternalTrigger.Value = true;
            }
        }

        internal void Reset(UserControl content)
        {
            if (content.IsNull() || content.GetType() != typeof(DevicePropertiesV))
                return;

            if(!saveIsRun)
            {
                triggerDC.IsInternalTrigger.Value = false;
                triggerDC.WaitToStop();
                triggerDC.Delay.Value = saveDelay;
            }
        }
    }
}
