use std::{fs::{self, remove_dir_all, create_dir}, path::PathBuf};

use acme_lib::{DirectoryUrl, persist::FilePersist, Directory, create_p384_key};
use serde::Deserialize;

#[derive(Debug, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Settings {
    pub account: String,
    pub domains: Vec<String>,
    pub productive: bool
}

fn main() {
    println!("Starting letsencrypt-update...");    

    let cert_dir = dirs::config_dir().expect("Could not find config dir")
        .join("letsencrypt-update");

    let settings = fs::read_to_string(cert_dir.join("letsencrypt-update.conf"))
        .expect("Could not read settings");
    let settings: Settings = serde_json::from_str(&settings).expect("Could not extract settings");
    println!("Settings: {settings:#?}");

    let url = if settings.productive { 
        DirectoryUrl::LetsEncrypt 
    } else { 
        DirectoryUrl::LetsEncryptStaging 
    };
    
    // Save/load keys and certificates to current dir.
    let persist = FilePersist::new(&cert_dir);

    // Create a directory entrypoint.
    let dir = Directory::from_url(persist, url).expect("Could not create directory entrypoint");    

    // Reads the private account key from persistence, or
    // creates a new one before accessing the API to establish
    // that it's there.

    let mut domains: Vec<&str> = settings.domains.iter().map(|n| { 
        let str: &str = n; 
        str
    }).collect();

    domains.sort();
    let acc = dir.account(&settings.account).expect("Could not read key from persistence");
    if let Some(cert) = acc.certificate(&domains[0]).ok().flatten() {
        println!("valid days left: {}", cert.valid_days_left());
        if cert.valid_days_left() > 30 { 
            println!("Certificate exists and is valid, exiting...");
            return 
        }
    }

    // Order a new TLS certificate for a domain.
    let mut ord_new = acc.new_order(&domains[0], &domains).expect("Could not create new order");

    // If the ownership of the domain(s) have already been
    // authorized in a previous order, you might be able to
    // skip validation. The ACME API provider decides.
    let ord_csr = loop {
        // are we done?
        if let Some(ord_csr) = ord_new.confirm_validations() {
            break ord_csr;
        }

        // Get the possible authorizations (for a single domain
        // this will only be one element).
        //let auths = ord_new.authorizations()?;
        let auths = ord_new.authorizations().expect("Could not get authorizations");
        auths.iter().for_each(|auth| {
            // For HTTP, the challenge is a text file that needs to
            // be placed in your web server's root:
            //
            // /var/www/.well-known/acme-challenge/<token>
            //
            // The important thing is that it's accessible over the
            // web for the domain(s) you are trying to get a
            // certificate for:
            //
            // http://mydomain.io/.well-known/acme-challenge/<token>
            let chall = auth.http_challenge();   
            
            // The token is the filename.
            let token = chall.http_token();

            let acme_challenge = &cert_dir.join("acme-challenge");
            let _ = remove_dir_all(&acme_challenge);
            create_dir(&acme_challenge).expect("Could ot create acme-challenge dir");

            // The proof is the contents of the file
            let proof = chall.http_proof();
            fs::write(acme_challenge.join(token), &proof).expect("Unable to write file");        
            // The order at ACME will change status to either
            // confirm ownership of the domain, or fail due to the
            // not finding the proof. To see the change, we poll
            // the API with 5000 milliseconds wait between.
            //chall.validate(5000)?;
            chall.validate(5000).expect("Could not validate cert");
        });
        // Update the state against the ACME API.
        ord_new.refresh().expect("Could not refresh order");
    };

    // Ownership is proven. Create a private key for
    // the certificate. These are provided for convenience, you
    // can provide your own keypair instead if you want.
    let pkey_pri = create_p384_key();

    // Submit the CSR. This causes the ACME provider to enter a
    // state of "processing" that must be polled until the
    // certificate is either issued or rejected. Again we poll
    // for the status change.
    let ord_cert = ord_csr.finalize_pkey(pkey_pri, 5000).expect("Could not submit CSR");

    // Now download the certificate. Also stores the cert in
    // the persistence.
    let cert = ord_cert.download_and_save_cert().expect("Could not download certificate");
    let cert_str = cert.certificate();
    write_cert(&cert_dir, cert_str);
    let key_str = cert.private_key();
    write_cert(&cert_dir, key_str);

    println!("Finished")
}

fn write_cert(cert_dir: &PathBuf, cert: &str) {

}