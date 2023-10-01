namespace Tools.Framework
{
	public class BaseClass
	{
		private DDebug _logger;
		protected DDebug log
			=> _logger ??= new DDebug(GetType().Name);
	}
}