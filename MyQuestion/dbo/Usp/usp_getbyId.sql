CREATE PROCEDURE [dbo].[usp_getbyId]
	@id int 
AS
select *from Question where Id = @Id;
GO