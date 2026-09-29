 backend "s3" {
    bucket                      = "cc-demo-tfstate"
    key                         = "cc-demo/terraform.tfstate"
    region                      = "auto"
    endpoints                   = { s3 = "https://b89c67dd628b04e9e40f2e30f7d1a419.r2.cloudflarestorage.com" }
    skip_credentials_validation = true
    skip_region_validation      = true
    skip_requesting_account_id  = true
    skip_s3_checksum            = true
    use_path_style              = true
  }
}
