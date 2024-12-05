
using System.Collections.Generic;

public class Leegality_docStatusClass
    {
    public DocumentData Data { get; set; }
    public List<LeegalityMessage> Messages { get; set; }
    public int Status { get; set; }
}
public class LeegalityMessage // Renamed class
{
    public string Code { get; set; }
    public string MessageText { get; set; }
}
public class DocumentData
{
    public string File { get; set; }
    public string AuditTrail { get; set; }
    public string DocumentId { get; set; }
    public string DocumentName { get; set; }
    public string WorkflowId { get; set; }
    public string WorkflowName { get; set; }
    public string Irn { get; set; }
    public string Status { get; set; }
    public string CreationDate { get; set; }
    public string CompletionDate { get; set; }
    public string DeletionDate { get; set; }
    public string SenderName { get; set; }
    public string SenderUsername { get; set; }
    public List<Invitees> Invitations { get; set; }
}

public class Invitees
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string SignUrl { get; set; }
    public bool Active { get; set; }
    public bool Signed { get; set; }
    public bool Rejected { get; set; }
    public bool Expired { get; set; }
    public string CreationDate { get; set; }
    public string ExpiryDate { get; set; }
    public string SignDate { get; set; }
    public string SignType { get; set; }
}
