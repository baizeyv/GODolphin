using System.Reflection;

namespace GODolphin.AOP;

public interface IInvocationHandler
{
    /// <summary>
    /// * 预处理
    /// </summary>
    void Preprocess();

    /// <summary>
    /// * 执行部分
    /// </summary>
    /// <param name="proxy"></param>
    /// <param name="method"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    object Invoke(object proxy, MethodInfo method, object[] args);

    /// <summary>
    /// * 后处理
    /// </summary>
    void PostProcess();
}
