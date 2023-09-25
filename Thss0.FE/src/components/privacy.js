import { useNavigate } from 'react-router-dom'

function Privacy() {
    const navigate = useNavigate()
    return (
        <>
            <p>What is a Privacy Policy?
                <br />
                A Privacy Policy is a statement or a legal document that states how a company or website collects, handles and processes data of its customers and visitors. It explicitly describes whether that information is kept confidential, or is shared with or sold to third parties.
                <br />
                <br />
                Personal information about an individual may include the following:
                <br />
                <br />
                Name
                <br />
                Address
                <br />
                Email
                <br />
                Phone number
                <br />
                Age
                <br />
                Sex
                <br />
                Marital status
                <br />
                Race
                <br />
                Nationality
                <br />
                Religious beliefs
                <br />
                For example, an excerpt from Pinterest's Privacy Policy agreement clearly describes the information Pinterest collects from its users as well as from any other source that users enable Pinterest to gather information from. The information that the user voluntarily gives includes names, photos, pins, likes, email address, and/or phone number etc., all of which is regarded as personal information.
                <br />
                <br />
                Pinterest Privacy Policy: When you give it to us or give us permission to obtain it personal information clause
                <br />
                <br />
                Additionally, Pinterest also states that it collects user location data from mobile devices, and if someone makes a purchase on Pinterest, payment and contact information - including an address and phone number - will be collected. If users buy products or services for others, Pinterest gathers their contact information and shipping details, too.
                <br />
                <br />
                Users may also give Pinterest permission to access information that is shared with other websites like Facebook and Twitter by linking their Pinterest account with them. This information would also include information about their friends and followers. The account settings have information about how much access Pinterest has to their users' data.
                <br />
                <br />
                In sum, a Privacy Policy is where you let your users know all about how you make sure their privacy is respected by your business practices.
            </p>
            <button onClick={() => navigate(-1)} className="btn btn-outline-dark border-0 border-bottom rounded-0 col-2">Back</button>
        </>
    )
}
export default Privacy