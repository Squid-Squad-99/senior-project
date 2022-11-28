using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Blockchain
{
    public class PythonRunner : MonoBehaviour
    {
        public string Result => _result;

        [SerializeField] private string _pythonEnginePath;
        [SerializeField] private string _filePath;
        [SerializeField] private string[] _args;

        private Process _process;
        private ConcurrentQueue<string> _sendDataQueue = new();
        private ConcurrentQueue<string> _recvedDataQueue = new();
        private string _result;


        public void Run()
        {
            Task.Factory.StartNew(() =>
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _pythonEnginePath,
                    Arguments = string.Join(" ", _filePath, _args),
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                };

                using (_process = Process.Start(startInfo))
                {
                    // send data
                    while (_process != null)
                    {
                        while (_sendDataQueue.IsEmpty == false && _sendDataQueue.TryDequeue(out string data))
                        {
                            _process.StandardInput.WriteLine(data);
                            string result = _process.StandardOutput.ReadLine();
                            // save to result
                            _recvedDataQueue.Enqueue(result);
                        }

                        Thread.Sleep(16);
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        public IEnumerator Send(string data)
        {
            _sendDataQueue.Enqueue(data);
            yield return new WaitUntil(() => _recvedDataQueue.IsEmpty == false);
            _recvedDataQueue.TryDequeue(out _result);
        }
    }
}