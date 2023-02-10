

namespace AppFramework
{
    public enum EventDef
    {
        None,
        //UI
        UI_Show_AppStartupPanel,
        UI_CheckResourcesUpdateProgress,
        UI_CheckResourcesUpdateCompeleted,
        UI_Show_SplashPanel,
        UI_Destroy_SplashPanel,
        //Net
        Net_ReqLogin,
        Net_RspLoginSuccess
    }
}
