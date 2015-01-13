using System;

namespace ImageBrowser.Data
{
    /// <summary>
    /// Implementation of the RssSchema class.
    /// </summary>
    public class RssSchema : BindableSchemaBase, IEquatable<RssSchema>, IComparable<RssSchema>, IListItem
    {
        private string title;
        private string summary;
        private string content;
        private string imageUrl;
        private string extraImageUrl;
        private string mediaUrl;
        private string feedUrl;
        private string author;
        private DateTime publishDate;
        private int index;

        public string Id { get; set; }

        public int Index
        {
            get { return index; }
            set { SetProperty(ref index, value); }
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public string Summary
        {
            get { return summary; }
            set { SetProperty(ref summary, value); }
        }

        public string Content
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }

        public string ImageUrl
        {
            get { return imageUrl; }
            set { SetProperty(ref imageUrl, value); }
        }

        public string ExtraImageUrl
        {
            get { return extraImageUrl; }
            set { SetProperty(ref extraImageUrl, value); }
        }

        public string MediaUrl
        {
            get { return mediaUrl; }
            set { SetProperty(ref mediaUrl, value); }
        }

        public string FeedUrl
        {
            get { return feedUrl; }
            set { SetProperty(ref feedUrl, value); }
        }

        public string Author
        {
            get { return author; }
            set { SetProperty(ref author, value); }
        }

        public DateTime PublishDate
        {
            get { return publishDate; }
            set { SetProperty(ref publishDate, value); }
        }

        public override string DefaultTitle
        {
            get { return Title; }
        }

        public override string DefaultSummary
        {
            get { return Summary; }
        }

        public override string DefaultImageUrl
        {
            get { return ImageUrl; }
        }

        public override string DefaultContent
        {
            get { return Content; }
        }

        override public string GetValue(string fieldName)
        {
            if (!String.IsNullOrEmpty(fieldName))
            {
                switch (fieldName.ToLower())
                {
                    case "id":
                        return String.Format("{0}", Id);
                    case "title":
                        return String.Format("{0}", Title);
                    case "summary":
                        return String.Format("{0}", Summary);
                    case "content":
                        return String.Format("{0}", Content);
                    case "imageurl":
                        return String.Format("{0}", ImageUrl);
                    case "extraimageurl":
                        return String.Format("{0}", ExtraImageUrl);
                    case "mediaurl":
                        return String.Format("{0}", MediaUrl);
                    case "feedurl":
                        return String.Format("{0}", FeedUrl);
                    case "author":
                        return String.Format("{0}", Author);
                    case "publishdate":
                        return String.Format("{0}", PublishDate);
                    case "defaulttitle":
                        return String.Format("{0}", DefaultTitle);
                    case "defaultsummary":
                        return String.Format("{0}", DefaultSummary);
                    case "defaultimageurl":
                        return String.Format("{0}", DefaultImageUrl);
                    default:
                        break;
                }
            }
            return String.Empty;
        }

        public bool Equals(RssSchema other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;

            return this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RssSchema);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public int CompareTo(RssSchema other)
        {
            return -1 * this.PublishDate.CompareTo(other.PublishDate);
        }
    }
}
