using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ApiApplication {
  class Auth {
    public string Nonce {get; set;}
    public string Secret {get; set;}

    public string ApiKey {get; set;}
    public Auth()
    {
      this.Secret = Settings.Secret;
      this.ApiKey = Settings.ApiKey;
    }
    public Auth(string secret, string api_key)
    {
      this.Secret = secret;
      this.ApiKey = api_key;
    }

    public string Signature (string nonce)
    {
      ASCIIEncoding encoding = new ASCIIEncoding();
      byte[] secret = encoding.GetBytes(this.Secret);
      byte[] current_nonce = encoding.GetBytes(nonce);
      HMACSHA256 hmac = new HMACSHA256(secret);
      byte[] signature = hmac.ComputeHash(current_nonce);
      return BitConverter.ToString(signature).Replace("-", "").ToLower();
    }
  }
}
