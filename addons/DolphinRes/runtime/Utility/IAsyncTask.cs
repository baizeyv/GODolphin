using System;
using System.Threading.Tasks;

namespace GODolphin.Res;

public interface IAsyncTask
{
    public Task DoLoadAsync(System.Action callback);
}
