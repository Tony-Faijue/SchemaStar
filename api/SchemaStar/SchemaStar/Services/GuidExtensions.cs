namespace SchemaStar.Services
{
    public static class GuidExtensions
    {
        /// <summary>
        /// Converts a GUID to byte array for MySQL storage
        /// Handles .NET's little-endian GUID format
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static byte[] ToMySqlBinary(this Guid guid) 
        {
            var bytes = guid.ToByteArray();

            //Swap to big-endian for MySQL compatability
            Array.Reverse(bytes, 0, 4);
            Array.Reverse(bytes, 4, 2);
            Array.Reverse(bytes, 6, 2);
            return bytes;
        }

        /// <summary>
        /// Converts MySQL byte array back to GUID
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Guid ToGuidFromMySqlBinary(this byte[] bytes) 
        {
            if (bytes == null || bytes.Length != 16) 
            {
                throw new ArgumentException("Invalid binary data for GUID");
            }
            var copy = (byte[])bytes.Clone();
            //Reverse the swaps that was done when storing
            Array.Reverse(copy, 0, 4);
            Array.Reverse(copy, 4, 2);
            Array.Reverse(copy, 6, 2);

            return new Guid(copy);
        }
    }
}
