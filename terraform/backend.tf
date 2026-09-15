#   3. Fill in account_id/bucket below, and export the R2 keys as
#      AWS_ACCESS_KEY_ID / AWS_SECRET_ACCESS_KEY when running `terraform init`
#      (locally and as GitHub Actions secrets) - the S3 backend reads them
#      from the standard AWS env vars.
#
# Backend blocks can't reference variables, so these two values are literal.
terraform {
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
