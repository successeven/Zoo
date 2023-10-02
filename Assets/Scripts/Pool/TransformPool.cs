using UnityEngine;

namespace Tools.Pool
{
  public class TransformPool : BasePool<Transform>
  {
    public struct Ctx { }

    private readonly Ctx _ctx;

    public TransformPool(Ctx ctx, BaseCtx baseCtx) : base(baseCtx) { }
  }
}