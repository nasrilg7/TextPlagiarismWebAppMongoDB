using MongoDB.Bson;
using MongoDB.Driver;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TextPlagiarismWebApp.DataAccessLayer;
using TextPlagiarismWebApp.Models.ViewModels;

namespace TextPlagiarismWebApp.Models
{
    public class DocumentsLayer
    {
        private static DocumentsLayer documentsLayer;
        private int documentSentencesCount;
        private List<Sentence> testedSentences;
        private PgContext context;
        public static DocumentsLayer getInstance()
        {
            if (documentsLayer == null)
            {
                documentsLayer = new DocumentsLayer();
            }
            return documentsLayer;
        }

       

        public PgContext getContext()
        {
            //var builder = new NpgsqlConnectionStringBuilder();
            //builder.Host = "localhost";
            //builder.Database = "plagiarism_db";
            //builder.Port = 5432;
            //builder.Username = "postgres";
            //builder.Password = "postgres";
            //var factory = new NpgsqlConnectionFactory();
            //context = new PgContext(factory.CreateConnection(builder.ToString()), true);
            //return context;
            return null;

        }
        private IMongoDatabase getDatabase()
        {
            var client = new MongoClient();
            var database = client.GetDatabase("SentenceMatchingdb");
            return database;

            //var builder = new NpgsqlConnectionStringBuilder();
            //builder.Host = "localhost";
            //builder.Database = "plagiarism_db";
            //builder.Port = 5432;
            //builder.Username = "postgres";
            //builder.Password = "postgres";
            //var connection = new NpgsqlConnection(builder.ToString());
            //return connection;
        }
        public string getDocumentPath(ObjectId id)
        {
            var dl = getInstance();
            var filter = Builders<BsonDocument>.Filter.Eq("document_id", id.ToString());
            var doc = dl.getSentencesCollection().Find(filter).First();
            return doc["document_path"].AsString;
        }


        public List<BsonDocument> getSentencesByDocumentId(string id)
        {
            //DocumentsLayer dl = getInstance();
            //var connection = getConnection();
            //connection.Open();
            //var cmd = new NpgsqlCommand();
            //cmd.Connection = connection;
            //NpgsqlDataReader dr;
            //cmd.CommandText = "Select * from \"Sentences\" where \"Sentences\".\"DID\" = "+id+";";
            //dr = cmd.ExecuteReader();
            //var count = 0;
            //List<Sentence> sentencesList = new List<Sentence>();
            //while (dr.Read())
            //{
            //    Sentence sen = new Sentence();
            //    sen.Id = (int)dr[0];
            //    sen.sentence= (string)dr[1];
            //    sen.DID= (int)dr[2];
            //    count++;
            //    sentencesList.Add(sen);
            //}

            //dr.Close();


            //var plagiarisedContent = (double)count / sentencesList.Count * 100;
            //connection.Close();

            //return sentencesList;

            var dl = getInstance();
            var objId = ObjectId.Parse(id);
            var filter = Builders<BsonDocument>.Filter.Eq("document_id", objId);
            var sentences = dl.getSentencesCollection().Find(filter).ToList();
            return sentences;

        }
        public BsonDocument getSentenceBySentenceId(string id)
        {
            var dl = getInstance();

            var objId = ObjectId.Parse(id);
            var filter = Builders<BsonDocument>.Filter.Eq("sentence_id", objId);
            var sentence = dl.getSentencesCollection().Find(filter).First();
            return sentence;

        }

        public List<SentenceMatchingViewModel> getMatchedSentencesByDocumentId(string id, string path)
        {
            //    DocumentsLayer dl = getInstance();
            //    var connection = getConnection();
            //    connection.Open();
            //    var cmd = new NpgsqlCommand();
            //    cmd.Connection = connection;
            //    NpgsqlDataReader dr;
            //    var count = 0;
            //    var sentencesList = getSentencesByDocumentId(id);
            //    List<SentenceMatchingViewModel> matchedSentencesList = new List<SentenceMatchingViewModel>();
            //    foreach (var item in sentencesList)
            //    {
            //        var m = NpgsqlTextFunctions.PlainToTsQuery(item.sentence);
            //        cmd.CommandText = "SELECT \"Sentences\".\"sentence\",\"Sentences\".\"Id\", ts_rank(\"Sentences\".\"VectorValue\", "+m+") FROM \"Documents\", \"Sentences\" Where \"Documents\".\"Id\" = \"Sentences\".\"DID\" AND ts_rank(\"Sentences\".\"VectorValue\", "+ m + ") > 0.7 AND \"Sentences\".\"DID\" != " + id + " LIMIT 1;";
            //        dr = cmd.ExecuteReader();

            //        while (dr.Read())
            //        {
            //            SentenceMatchingViewModel sen = new SentenceMatchingViewModel();
            //            sen.Id = item.Id;
            //            sen.sentence = item.sentence;
            //            sen.sentencesCount = sentencesList.Count;
            //            sen.similarity = (dr[2].ToString());
            //            sen.documentId = id;
            //            sen.matchedSentenceId = (int)dr[1];
            //            sen.matchedSentence = (string)dr[0];
            //            count++;
            //            matchedSentencesList.Add(sen);
            //        }

            //        dr.Close();
            //    }
            //    connection.Close();

            //    return matchedSentencesList;
            var dl = getInstance();
            var did = new ObjectId(id);
            var allSentencesInThatDocument = readSentences(path, did);
            var matchedSentencesList = new List<SentenceMatchingViewModel>();
            documentSentencesCount = allSentencesInThatDocument.Count;
            foreach (var sentenceInDocument in allSentencesInThatDocument)
            {
                
                try
                {
                    var filterBuilder = Builders<BsonDocument>.Filter;
                    var filter = filterBuilder.Text(sentenceInDocument.sentence);
                    var projection = Builders<BsonDocument>.Projection.MetaTextScore("textScore");
                    var sort = Builders<BsonDocument>.Sort.MetaTextScore("textScore");
                    var doc = dl.getSentencesCollection().Find(filter).Project(projection).Sort(sort).First();
                    var matchedSentence = new SentenceMatchingViewModel();
                    if (doc["textScore"].AsDouble >= 3.5)
                    {
                        matchedSentence.Id = sentenceInDocument.Id.ToString();
                        matchedSentence.sentence = sentenceInDocument.sentence;
                        matchedSentence.documentId = doc["document_id"].AsObjectId.ToString();
                        matchedSentence.documentPath = doc["document_path"].AsString;
                        matchedSentence.matchedSentenceId = doc["sentence_id"].AsObjectId.ToString();
                        matchedSentence.matchedSentence = doc["sentence"].AsString;
                        matchedSentencesList.Add(matchedSentence);
                    }

                    
                }
                catch(Exception e)
                {
                    //handle the exception
                }
                
            }

            dl.insertSentences(allSentencesInThatDocument);
            return matchedSentencesList;
        }
        public int getDocumentSentencesCount()
        {
            return documentSentencesCount;
        }


        public List<Document> getDocuments()
        {
            //DocumentsLayer dl = getInstance();
            //var context = dl.getContext();
            //return context.Documents.ToList();
            return null;
        }

        //public List<Sentence> getSentences()
        //{
        //    //DocumentsLayer dl = getInstance();
        //    //var context = dl.getContext();
        //    //return context.Sentences.ToList();
        //    return null;
        //}
        public void insertDocument(Document document)
        {
            //DocumentsLayer dl = getInstance();
            //var context = dl.getContext();
            //context.Documents.Add(document);
            //context.SaveChanges();

            DocumentsLayer dl = getInstance();
            var bson_document = new BsonDocument
                {
                    { "document_id", document.Id},
                    { "document_path", document.path },
                    { "document", document.document }
                };
            dl.getDocumentsCollection().InsertOne(bson_document);

        }

        public void insertSentences(List<Sentence> sentencesList)
        {
            //DocumentsLayer dl = getInstance();
            //var context = dl.getContext();
            //foreach (var sen in sentencesList)
            //{
            //    sen.DID = id;
            //    context.Sentences.Add(sen);
            //}
            //context.SaveChanges();

            DocumentsLayer dl = getInstance();
            
            foreach (var sen in sentencesList)
            {

                var bson_document = new BsonDocument
                {
                    { "sentence_id", sen.Id},
                    { "document_id", sen.DID },
                    { "document_path", sen.path },
                    { "sentence", sen.sentence }
                };

                dl.getSentencesCollection().InsertOne(bson_document);
            }
        }

        public void setSentences(List<Sentence> sentencesList)
        {
            testedSentences = sentencesList;
        }
        public List<Sentence> getSentences()
        {
            return testedSentences;
        }


        public IMongoCollection<BsonDocument> getDocumentsCollection()
        {
            DocumentsLayer dl = getInstance();

            var document_collection = dl.getDatabase().GetCollection<BsonDocument>("documents");
            return document_collection;

        }
        public IMongoCollection<BsonDocument> getSentencesCollection()
        {
            DocumentsLayer dl = getInstance();

            var sentence_collection = dl.getDatabase().GetCollection<BsonDocument>("sentences");
            return sentence_collection;

        }
        private static List<Sentence> readSentences(string filePath, ObjectId did)
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            object miss = System.Reflection.Missing.Value;
            object path = @"" + filePath;
            object readOnly = true;
            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
            List<Sentence> sentencesList = new List<Sentence>();
            for (int i = 0; i < docs.Sentences.Count; i++)
            {
                Sentence sen = new Sentence();
                sen.sentence = docs.Sentences[i + 1].Text.ToString();
                sen.path = filePath;
                sen.DID = did;
                sentencesList.Add(sen);
            }
            docs.Close();
            word.Quit();
            return sentencesList;


        }
    }
}