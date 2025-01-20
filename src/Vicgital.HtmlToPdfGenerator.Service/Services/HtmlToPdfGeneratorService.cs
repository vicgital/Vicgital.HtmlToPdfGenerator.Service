using Google.Protobuf;
using Grpc.Core;
using Vicgital.Core.Configuration.Services;
using Vicgital.HtmlToPdfGenerator.Business.Components.Definition;
using Vicgital.HtmlToPdfGenerator.Service.Definition;

namespace Vicgital.HtmlToPdfGenerator.Service.Services
{
    public class HtmlToPdfGeneratorService(
        ILogger<HtmlToPdfGeneratorService> logger,
        IAppConfigurationService appConfigurationService,
        IPdfGeneratorComponent pdfGeneratorComponent) : Definition.HtmlToPdfGenerator.HtmlToPdfGeneratorBase
    {
        
        private readonly ILogger<HtmlToPdfGeneratorService> _logger = logger;
        private readonly IAppConfigurationService _appConfigurationService = appConfigurationService;
        private readonly IPdfGeneratorComponent _pdfGeneratorComponent = pdfGeneratorComponent;
        

        public async override Task ConvertHtmlToPDF(ConvertHtmlToPDFRequest request, IServerStreamWriter<Chunk> responseStream, ServerCallContext context)
        {

            if (string.IsNullOrEmpty(request.BodyHtml))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "BodyHtml is required"));

            try
            {
                var document = _pdfGeneratorComponent.ConvertHtmlToPDF(request.BodyHtml, request.HeaderHtml, request.FooterHtml);
                await DocumentStreamInteral(document, responseStream);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error - ConvertHtmlToPDF()");
                throw new RpcException(new Status(StatusCode.Internal, $"Error ConvertHtmlToPDF() - Message: {ex.Message}"));
            }
            
        }

        internal async Task DocumentStreamInteral(byte[] document, IServerStreamWriter<Chunk> responseStream)
        {
            if (document == null || document.Length == 0)
            {
                throw new KeyNotFoundException();
            }
            else
            {
                int chunkSize = int.Parse(_appConfigurationService.GetValue("FILE_STREAM_CHUNK_SIZE"));
                decimal chunkCount = Math.Ceiling(document.Length / (decimal)chunkSize);
                int offset = 0;

                for (int i = 0; i < chunkCount; i++)
                {
                    int bytesToTake = Math.Min(chunkSize, document.Length - i * chunkSize);
                    byte[] chunk = document.Skip(offset).Take(bytesToTake).ToArray();

                    await responseStream.WriteAsync(new Chunk
                    {
                        Data = ByteString.CopyFrom(chunk)
                    });

                    offset += (chunk.Length);
                }
            }
        }
    }
}
