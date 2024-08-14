SELECT * FROM Tag;

SELECT b.*, t.Id AS TagId, t.Name AS TagName, bt.Id AS BlogTagId, bt.BlogId, bt.TagId 
FROM Blog b
JOIN BlogTag bt ON bt.Id = b.Id
JOIN Tag t ON t.Id = bt.TagId
WHERE b.Id = bt.BlogId; 