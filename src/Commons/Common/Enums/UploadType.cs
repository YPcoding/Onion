using System.ComponentModel;

namespace Common.Enums;

public enum UploadType : byte
{
    [Description(@"Documents")]
    Document,
    [Description(@"Image")]
    Image
}
