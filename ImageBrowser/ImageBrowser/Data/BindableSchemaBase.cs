using System.Text;

namespace ImageBrowser.Data
{
  public abstract class BindableSchemaBase : BindableBase
  {
    public abstract string DefaultTitle { get; }

    public abstract string DefaultSummary { get; }

    public abstract string DefaultImageUrl { get; }

    public abstract string DefaultContent { get; }

    public abstract string GetValue(string propertyName);

    public virtual string GetValues(params string[] propertyNames)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string propertyName in propertyNames)
      {
        object obj = (object) (this.GetValue(propertyName) ?? string.Empty);
        stringBuilder.AppendLine(obj.ToString());
      }
      return stringBuilder.ToString();
    }
  }
}
