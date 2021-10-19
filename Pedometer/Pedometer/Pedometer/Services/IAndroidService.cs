using System;
using System.Collections.Generic;
using System.Text;

namespace Pedometer.Services
{
    public interface IAndroidService
    {
        void StartService();

        void StopService();

        void ResetService();
    }
}
