using System;

namespace LexisNexis.Red.iOS
{
	public static class ViewConstant
	{
		#region publication cover
		public const float PUBLICATION_VIEW_HEIGHT = 338f;
		public const float PUBLICATION_VIEW_WIDTH = 200f;

		public const float PUBLICATION_COVER_X = 0f;
		public const float PUBLICATION_COVER_Y = 0f;
		public const float PUBLICATION_COVER_WIDTH = 200f;
		public const float PUBLICATION_COVER_HEIGHT = 270f;
		public const float PUBLICATION_COVER_HORIZONTAL_SPACING = 30f;
		public const float PUBLICATION_COVER_LINE_PADDING = 10f;
		public const float PUBLICATION_COVER_LINE_TOP_VIEW_HEIGHT = 5.0f;

		public const float PUBLICATION_TITLE_LABEL_X = 15f;
		public const float PUBLICATION_TITLE_LABEL_Y = 30f;
		public const float PUBLICATION_TITLE_LABEL_WIDTH = 170f;
		public const float PUBLICATION_TITLE_LABEL_HEIGHT = 110f;
		public const int PUBLICATION_TITLE_LABEL_LINES = 4;

		public const float STATUS_LABEL_HEIGHT = 15f;
		public const float STATUS_LABEL_WIDTH = 150f;
		public const float STATUS_DATE_HEIGHT = 15f;
		public const float STATUS_DATE_WIDTH = 150f;
		#endregion


		#region badage of UIBarButtonItem
		public const float BADAGE_ORIGIN_X = 14f;
		public const float BADAGE_ORIGIN_Y = -10f;
		public const float BADAGE_SIZE_DIAMETER = 16f;
		public const float BADAGE_FONT_SIZE = 10f;
		#endregion


		#region publication info modal view
		public const float INFO_NAME_LABEL_FONT_SIZE = 22f;
		public const float INFO_AUTHOR_LABEL_FONT_SIZE = 14f;

		public const float INFO_AUTHOR_LABEL_WIDTH = 415f;
		public const float INFO_AUTHOR_LABEL_MIN_HEIGHT = 18f;
		public const float INFO_AUTHOR_LABEL_MAX_HEIGHT = 72f;//4 lines at most, so the max height is 4 * 18

		public const int INFO_NAME_AUTHOR_SCROLL_VIEW_MAX_HEIGHT = 200;
		public const int INFO_LOAN_TAG_LABEL_WIDTH = 42;
		#endregion

		#region tag
		public const int TAG_INNER_CIRCLE_RADIUS = 11;
		public const int TAG_OUTER_CIRCLE_RADIUS = 13;
		#endregion

		#region PDF
		public const string SHARE_TMP_PDF_NAME = "share_tmp.pdf";
		#endregion
	}
}

