
namespace MyBehaviorTree
{
    public enum AbortType
    {
        None,
        LowerPriority,//只要当前同等级Running的Task比我低，就继续子任务。
        Self,//只要当前还有同级task还在Running，就继续子任务。
        Both
    }
}