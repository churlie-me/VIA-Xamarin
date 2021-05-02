using System;
using System.Collections.Generic;
using System.Text;
using Via.Models;

namespace Via.Data
{
    public interface CompletedCallBackEvent<T>
    {
        void OnSuccess(T O);
        void OnFailure(string E);
        void OnFailure(Exception E);
        // void OnFailure(isAliveStatus status);
    }

    public interface ViaUsersCallbackEvent : CompletedCallBackEvent<List<ViaUser>> { }
}
