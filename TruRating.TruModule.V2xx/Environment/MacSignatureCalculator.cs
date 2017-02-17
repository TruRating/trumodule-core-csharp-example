// The MIT License
// 
// Copyright (c) 2017 TruRating Ltd. https://www.trurating.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TruRating.TruModule.V2xx.Environment
{
    public interface IMacSignatureCalculator
    {
        string Calculate(string payload, string activationKey);
    }

    public class MacSignatureCalculator : IMacSignatureCalculator
    {
        /// <summary>
        ///     Calculates MAC code (signature) for specified payload
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="activationKey">The activation key.</param>
        /// <returns></returns>
        /// <remarks>convenience overload to allow signature generation from payload string</remarks>
        public string Calculate(string payload, string activationKey)
        {
            //convert to stream
            var payloadStream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) {Position = 0};
            //makr sure stream is at start
            //generate sugnature
            var des = TripleDES.Create();
            des.Key = Encoding.UTF8.GetBytes(activationKey);
            des.Mode = CipherMode.ECB;
            var encryptor = des.CreateEncryptor();
            var hash = SHA256.Create().ComputeHash(payloadStream);
            var encryptedHash = encryptor.TransformFinalBlock(hash, 0, hash.Length);
            return BitConverter.ToString(encryptedHash).Replace("-", string.Empty);
        }
    }
}