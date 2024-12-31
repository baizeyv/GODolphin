using GODolphin.Pool;
using Godot;

// NOTE: 这个 ActionKit 是仿照 LibGDX 中的 Action 仿写的, 旨在实现简单动作以及类似 Unity DOTween 的效果

namespace GODolphin.Action
{
    public abstract class ActionExecutor : IPoolable
    {
        protected Pool<ActionExecutor> Pool;

        public abstract bool Act(double delta);

        public virtual void Restart() { }

        public virtual void Reset()
        {
            Pool = null;
            Restart();
        }

        public Pool<ActionExecutor> GetPool()
        {
            return Pool;
        }

        public void SetPool(Pool<ActionExecutor> pool)
        {
            Pool = pool;
        }

        public void Recycle()
        {
            Pool?.Free(this);
            Pool = null;
        }
    }
}
