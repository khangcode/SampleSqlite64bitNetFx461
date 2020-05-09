using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleSqlite64bitFx461
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "EmailDB.sqlite3");
            if (System.IO.File.Exists(sFile)) System.IO.File.Delete(sFile);
            var sql_con = new System.Data.SQLite.SQLiteConnection("Data Source="+ sFile + ";Version=3");
            sql_con.Open();
            sql_con.EnableExtensions(true);
            sql_con.LoadExtension("SQLite.Interop.dll", "sqlite3_fts5_init"); // Or "SQLite.Interop.dll" as you need.
            var sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = @"CREATE TABLE TMail  
                                    (MailId INTEGER PRIMARY KEY,
                                        SentOn  TEXT,
                                        ReceivedOn TEXT,
                                        Subject TEXT,
                                        Body TEXT,
                                        SenderEmailAddress  TEXT,
                                        SenderName TEXT,
                                        CTo TEXT,
                                        CC TEXT,
                                        AttachmentNames TEXT,
                                        Categories TEXT,
                                        Importance  TEXT,
                                        FlagRequest TEXT,
                                        CTag TEXT,
                                        LastWriteTime TEXT,
                                        FullFileName TEXT)";
            sql_cmd.ExecuteNonQuery();
            sql_cmd.CommandText = @"CREATE VIRTUAL TABLE VMail 
                                    USING fts5(MailId,
                                    SentOn,
                                    ReceivedOn,
                                    Subject,
                                    Body,
                                    SenderEmailAddress,
                                    SenderName,
                                    CTo,
                                    CC,
                                    AttachmentNames,
                                    Categories,
                                    Importance,
                                    FlagRequest,
                                    CTag,
                                    LastWriteTime,
                                    FullFileName)";
            sql_cmd.ExecuteNonQuery();
            sql_cmd.CommandText = @"CREATE TRIGGER TrigDelete
                                    AFTER DELETE
                                        ON TMail
                                    FOR EACH ROW
                                    BEGIN
                                        DELETE FROM VMail
                                        WHERE MailId = old.MailId;
                                    END;";
            sql_cmd.ExecuteNonQuery();
            sql_cmd.CommandText = @"CREATE TRIGGER TrigInsert
                                    AFTER INSERT
                                        ON TMail
                                    FOR EACH ROW
                                    BEGIN
                                        INSERT INTO VMail (
                                            MailId,
                                            SentOn,
                                            ReceivedOn,
                                            Subject,
                                            Body,
                                            SenderEmailAddress,
                                            SenderName,
                                            CTo,
                                            CC,
                                            AttachmentNames,
                                            Categories,
                                            Importance,
                                            FlagRequest,
                                            CTag,
                                            LastWriteTime,
                                            FullFileName)
                                     VALUES (
                                            new.MailId,
                                            new.SentOn,
                                            new.ReceivedOn,
                                            new.Subject,
                                            new.Body,
                                            new.SenderEmailAddress,
                                            new.SenderName,
                                            new.CTo,
                                            new.CC,
                                            new.AttachmentNames,
                                            new.Categories,
                                            new.Importance,
                                            new.FlagRequest,
                                            new.CTag,
                                            new.LastWriteTime,
                                            new.FullFileName);
                                    END;";
            sql_cmd.ExecuteNonQuery();
            sql_cmd.CommandText = @"CREATE TRIGGER TrigUpdate
                                   AFTER UPDATE
                                        ON TMail
                                   FOR EACH ROW
                                   BEGIN
                                        UPDATE VMail
                                        SET SentOn = new.SentOn,
                                            ReceivedOn = new.ReceivedOn,
                                            Subject = new.Subject,
                                            Body = new.Body,
                                            SenderEmailAddress = new.SenderEmailAddress,
                                            SenderName = new.SenderName,
                                            CTo = new.CTo,
                                            CC = new.CC,
                                            AttachmentNames = new.AttachmentNames,
                                            Categories = new.Categories,
                                            Importance = new.Importance,
                                            FlagRequest = new.FlagRequest,
                                            CTag = new.CTag,
                                            LastWriteTime = new.LastWriteTime,
                                            FullFileName = new.FullFileName
                                        WHERE MailId = old.MailId;
                                    END;";
            sql_cmd.ExecuteNonQuery();

            sql_cmd.CommandText = @"CREATE INDEX SentOnIdx ON TMail(SentOn);";
            sql_cmd.ExecuteNonQuery();

            sql_cmd.CommandText = @"CREATE INDEX ReceivedOnIdx ON TMail(ReceivedOn);";
            sql_cmd.ExecuteNonQuery();

            sql_cmd.CommandText = @"CREATE INDEX LastWriteTimeIdx ON TMail(LastWriteTime);";
            sql_cmd.ExecuteNonQuery();

            sql_cmd.CommandText = @"CREATE INDEX SenderEmailAddressIdx ON TMail(SenderEmailAddress);";
            sql_cmd.ExecuteNonQuery();

            sql_cmd.CommandText = @"CREATE INDEX FullFileNameIdx ON TMail(FullFileName);";
            sql_cmd.ExecuteNonQuery();

            sql_cmd.CommandText = @"CREATE TABLE TTag (TagId INTEGER PRIMARY KEY, TagName TEXT, IsDisable INTEGER)";
            sql_cmd.ExecuteNonQuery();

            sql_cmd.CommandText = @"CREATE TABLE TRelation (RelationId INTEGER PRIMARY KEY, RTagId INTEGER, RMailId INTEGER,
                                    FOREIGN KEY(RTagId) REFERENCES TTag(TagId)
                                    FOREIGN KEY(RMailId) REFERENCES TMail(MailId))";
            sql_cmd.ExecuteNonQuery();

            sql_cmd.CommandText = @"CREATE INDEX TagIdIdx ON TRelation(RTagId);";
            sql_cmd.ExecuteNonQuery();

            sql_con.Close();
            MessageBox.Show("done!");
        }
    }
}
