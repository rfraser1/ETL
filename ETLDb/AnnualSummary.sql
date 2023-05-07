CREATE VIEW [dbo].[AnnualSummary]
AS 
select session, YEAR(date) RecordYear, COUNT(*) DataCount
from Rainfall
group by session, YEAR(date)
