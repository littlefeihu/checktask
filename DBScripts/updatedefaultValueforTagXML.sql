ALTER TABLE [dbo].[tblUser] DROP   CONSTRAINT [DF_XML] 
go
ALTER TABLE [dbo].[tblUser] ADD  CONSTRAINT [DF_XML]  DEFAULT ('<AnnotationTags MaxID="5">
<AnnotationTag TagID="8B083B6D-8E58-463D-8CC0-3D662F2FDEAF"  SortOrder="2"  Color="#902790"><Title>Tag 1</Title></AnnotationTag>
<AnnotationTag TagID="6B758A0F-A6CA-41EC-B765-8AA6553BECE7"   SortOrder="1"  Color="#EB008D"><Title>Tag 2</Title></AnnotationTag>
<AnnotationTag TagID="56A6BC18-A1B8-4789-BDD3-0BE732D085C9" SortOrder="5" Color="#FC2A51"><Title>Tag 3</Title></AnnotationTag>
<AnnotationTag TagID="53AB149D-D884-4E1E-A47A-04743ED8557F"  SortOrder="3" Color="#FD9600"><Title>Tag 4</Title></AnnotationTag>
<AnnotationTag TagID="EBB771C0-E2F3-4952-839F-F6CB92CCABF2" SortOrder="4" Color="#44DB5F"><Title>Tag 5</Title></AnnotationTag>
</AnnotationTags>') FOR [AnnotationTags]
GO
