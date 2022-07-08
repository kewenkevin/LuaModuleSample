namespace ND.Managers.ResourceMgr.Runtime
{
    public enum ResState
    {
        /// <summary>
        /// 尚未开始加载
        /// </summary>
        waitForLoad,
        /// <summary>
        /// 加载中
        /// </summary>
        loading,
        /// <summary>
        /// 已经加载好
        /// </summary>
        loaded,
        /// <summary>
        /// 已经被卸载
        /// </summary>
        unloaded
    }
}