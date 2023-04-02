using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GoogleMapPostCode;
using System.Text.RegularExpressions;

namespace GoogleDistanceMatrixAPI
{
    class Program
    {


        //path to file
        static string path = @"c:\temp\historyLog.txt";


        //PARAMETERS TO CONSTRUCT URL REQUEST
        static HttpClient client = new HttpClient();
        static string base_Url = "https://maps.googleapis.com/maps/api/distancematrix/json?";

        //SETTING MODE TO WALK, ALTHOUGH DEFAULT IS DRIVE IF EXEMPTED
        static string mode = "mode=walking";
        static string units = "units=imperial";

        //SPECIFYING REGION TO BE UK
        static string region = "region=uk";

        //API KEY FOR AUTHORIZATION
        static string apiKey = "key=AIzaSyBFJrnzNTsgnAbJf5VCkUbPQjLJD5JI_u8";
        

        //VALIDATION FOR UK POSTCODE USING REGEX EXPRESSION
        static bool validatePostCode(string postCode)
        {
            Regex regex = new Regex(@"^[A-Z]{1,2}[0-9R][0-9A-Z]? [0-9][ABD-HJLNP-UW-Z]{2}$");

            return regex.IsMatch(postCode);
        }


        static void writeAndReadHistoryLog(string destination, string origin, string miles, int valuex)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("HISTORY LOG FOR DISTANCE BETWEEN POSTCODES CREATED");
                }
            }

            // NEXT WE APPEND THE INFORMATION GATHERED ON THE POSTCODES
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine("THE ORIGIN ADDRESS IS: " + " " + origin);
                sw.WriteLine("THE DESTINATION ADDRESS IS: " + " " + destination);
                sw.WriteLine("THE DISTANCE BETWEEN BOTH ADDRESSES IN MILES IS " + " " + miles);
                sw.WriteLine("THE DISTANCE BETWEEN BOTH ADDRESS IN UNIT SPECIFIED IS" + " " + valuex);
            }

            // Open the file to read from.
            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }

        }


        //ASYNC METHOD USING HTTPCLIENT TO MAKE REQUEST TO THE GOOGLE MAP DISTANCE MATRIX API USING OUR SPECIFIED KEY
        static async Task<string> fetchDistance(string postCodeOrigin, string postCodeDestination)
        {

            string origin = "origins="+postCodeOrigin;
            string URL = base_Url + origin + "&destinations=" + postCodeDestination + "&" + mode + "&" + units + "&" + region + "&" + apiKey;
            HttpResponseMessage response = await client.GetAsync(URL);
            String responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;


        }

        static void Main(string[] args)
        {
            Console.WriteLine("ENTER ORIGIN POSTCODE :");

            string firstpost = Console.ReadLine();

            Console.WriteLine("");

            Console.WriteLine("ENTER DESTINATION POSTCODE");

            string secondpost = Console.ReadLine();


            if (string.IsNullOrEmpty(firstpost) || string.IsNullOrEmpty(secondpost))
            {
                Console.WriteLine("Neither Postcodes can be null");

            }

            if (!(validatePostCode(firstpost) && validatePostCode(secondpost)))
            {
                Console.WriteLine("PostCode invalid");
            }


            //CALL THE METHOD TO FETCH DISTANCE USING GOOGLE MAPS

            string response = fetchDistance(firstpost, secondpost).GetAwaiter().GetResult();

            //DESERIALIZE OBJECT INTO MOCK MODEL CREATED
            distanceResponse modelObj = JsonConvert.DeserializeObject<distanceResponse>(response);

            //NOW WE HAVE THE INFORMATION THAT WE NEED, WE CAN APPEND INTO TEXT FILE "historyLog.txt"
            
            if((string.IsNullOrEmpty(modelObj.rows[0].elements[0].status)) || modelObj.rows[0].elements[0].status == "NOT_FOUND")
            {
                Console.WriteLine("REQUEST FAILED");

            }

            string destinationaddress = modelObj.destination_addresses[0];
            string originaddress = modelObj.origin_addresses[0];
            string milesdistance = modelObj.rows[0].elements[0].distance.text;
            int valuedistance = modelObj.rows[0].elements[0].distance.value;


            //CALLING OUR METHOD TO APPEND RESULT TO SPECIFIED PATH AND FILE
           writeAndReadHistoryLog(destinationaddress, originaddress, milesdistance, valuedistance);
           Console.ReadLine();

        }
    }
}