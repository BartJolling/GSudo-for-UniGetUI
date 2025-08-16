using gsudo.Commands;
using gsudo.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace gsudo
{
    class Program
    {
        async static Task<int> Main()
        {
            SymbolicLinkSupport.EnableAssemblyLoadFix();
            return await Start().ConfigureAwait(false);
        }

        private static async Task<int> Start()
        {
            ICommand cmd = null;

            try
            {
                var commandLine = ArgumentsHelper.GetRealCommandLine();
                var args = ArgumentsHelper.SplitArgs(commandLine);
                cmd = new CommandLineParser(args).Parse();

                if (cmd is null)
                {
                    Logger.Instance.Log("Commandline does not contain and option or a verb", LogLevel.Debug);
                    return 0;
                };

#if !DISABLE_INTEGRITY
                cmd.CheckIntegrity();
#endif

                return await cmd.Execute().ConfigureAwait(false);

            }
            catch (ApplicationException ex)
            {
                Logger.Instance.Log(ex.Message, LogLevel.Error); // one liner errors.
                return Constants.GSUDO_ERROR_EXITCODE;
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString(), LogLevel.Error); // verbose errors.
                return Constants.GSUDO_ERROR_EXITCODE;
            }
            finally
            {
                (cmd as IDisposable)?.Dispose();

                if (InputArguments.KillCache)
                {
                    await new KillCacheCommand(verbose: false).Execute().ConfigureAwait(false);
                }

                try
                {
                    // cleanup console before returning.
                    Console.CursorVisible = true;
                    Console.ResetColor();
                    await Task.Delay(1).ConfigureAwait(false); // force reset color on WSL.

                    if (InputArguments.Debug && !Console.IsInputRedirected && cmd?.GetType() == typeof(ServiceCommand))
                    {
                        Console.WriteLine("Service shutdown. This window will close in 10 seconds");
                        Thread.Sleep(10000);
                    }
                }
                catch { }
            }
        }
    }
}
