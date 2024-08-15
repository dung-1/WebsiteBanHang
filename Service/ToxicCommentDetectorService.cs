using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebsiteBanHang.Service
{
    public class ToxicCommentDetectorService
    {
        private InferenceSession _session;

        public ToxicCommentDetectorService(string modelPath)
        {
            _session = new InferenceSession(modelPath);
        }

        public bool IsToxicComment(string comment)
        {
            var inputMeta = _session.InputMetadata;
            var inputName = inputMeta.Keys.First();

            // Tiền xử lý bình luận thành tensor
            var inputTensor = PreprocessComment(comment, inputMeta[inputName].Dimensions);

            var inputData = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
            };

            try
            {
                using (var results = _session.Run(inputData))
                {
                    var probabilities = results.Last().AsTensor<float>().ToArray();
                    var probabilityOfToxic = probabilities[1];  // Lớp 1 là lớp tiêu cực
                    return probabilityOfToxic >= 0.5f;  // Giả sử ngưỡng là 0.5
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running inference: {ex.Message}");
                return false; // Xử lý lỗi theo nhu cầu của bạn
            }
        }

        private DenseTensor<float> PreprocessComment(string comment, IReadOnlyList<int> inputDimensions)
        {
            // Ví dụ: Chuyển đổi bình luận thành một vector số float phù hợp với kích thước đầu vào của model
            var maxLength = inputDimensions[1]; // Giả sử kích thước đầu vào là [batch_size, max_length]
            var tokens = Encoding.UTF8.GetBytes(comment).Select(b => (float)b).ToArray();

            // Đảm bảo tensor có kích thước phù hợp
            var tensorData = new float[maxLength];
            for (int i = 0; i < maxLength; i++)
            {
                tensorData[i] = i < tokens.Length ? tokens[i] : 0; // Padding với 0 nếu độ dài comment ngắn hơn max_length
            }

            return new DenseTensor<float>(tensorData, new[] { 1, maxLength });
        }
    }
}
