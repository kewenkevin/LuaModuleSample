using System.Collections.Generic;

namespace ND.Managers.ResourceMgr.Runtime
{
    public class ResGCReleaseAgent
    {
        public Queue<Res> m_waitReleaseRes = new Queue<Res>();
        
        public Queue<ResLoader> m_waitRecycleResloader = new Queue<ResLoader>();

        public void Release(Res res)
        {
            lock (m_waitReleaseRes)
            {
                m_waitReleaseRes.Enqueue(res);
            }
        }

        public void RecycleLoader(ResLoader loader)
        {
            lock (m_waitRecycleResloader)
            {
                m_waitRecycleResloader.Enqueue(loader);
            }
        }


        public void UnloadReal()
        {
            lock (m_waitReleaseRes)
            {
                while (m_waitReleaseRes.Count > 0)
                {
                    Res res = m_waitReleaseRes.Dequeue();
                    res.Unload();
                }
            }

            lock (m_waitRecycleResloader)
            {
                while (m_waitRecycleResloader.Count > 0)
                {
                    ResLoader res = m_waitRecycleResloader.Peek();
                    if (res.loadingCount<=0)
                        m_waitRecycleResloader.Dequeue().Recycle2Cache();
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}