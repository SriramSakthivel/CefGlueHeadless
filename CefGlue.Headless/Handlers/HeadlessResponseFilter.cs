using Xilium.CefGlue;

namespace CefGlue.Headless.Handlers;

public class HeadlessResponseFilter : CefResponseFilter
{
    private readonly HeadlessResponse response;
    private readonly TaskCompletionSource<HeadlessResponse> tcs;
    private readonly MemoryStream memoryStream = new MemoryStream();


    public HeadlessResponseFilter(HeadlessResponse response, TaskCompletionSource<HeadlessResponse> tcs)
    {
        this.response = response ?? throw new ArgumentNullException(nameof(response));
        this.tcs = tcs ?? throw new ArgumentNullException(nameof(tcs));
    }

    protected override bool InitFilter()
    {
        return true;
    }

    //protected override CefResponseFilterStatus Filter(UnmanagedMemoryStream dataIn, long dataInSize, out long dataInRead,
    //    UnmanagedMemoryStream dataOut, long dataOutSize, out long dataOutWritten)
    //{
    //    var maxWrite = Math.Min(dataInSize, dataOutSize);
    //    dataIn.Read(_buffer, 0, (int)maxWrite);
    //    dataOutWritten = maxWrite;

    //    // You can log or process the data here
    //    var str = Encoding.UTF8.GetString(_buffer, 0, (int)maxWrite);
    //    Console.WriteLine("Received data: " + str);

    //    dataInRead = dataInSize;
    //    return CefResponseFilterStatus.Done;
    //}

    protected override CefResponseFilterStatus Filter(UnmanagedMemoryStream dataIn, long dataInSize, out long dataInRead,
        UnmanagedMemoryStream dataOut, long dataOutSize, out long dataOutWritten)
    {
        if (dataIn == null)
        {
            dataInRead = 0;
            dataOutWritten = 0;

            response.ResponseBytes = null;
            tcs.TrySetResult(response);

            return CefResponseFilterStatus.Done;
        }

        dataInRead = dataIn.Length;
        dataOutWritten = Math.Min(dataInRead, dataOut.Length);

        //Important we copy dataIn to dataOut
        dataIn.CopyTo(dataOut);

        //Copy data to stream
        dataIn.Position = 0;
        dataIn.CopyTo(memoryStream);

        response.ResponseBytes = memoryStream.ToArray();
        tcs.TrySetResult(response);

        return CefResponseFilterStatus.Done;
    }

    public byte[] Data => memoryStream.ToArray();
}