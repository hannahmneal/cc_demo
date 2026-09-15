terraform {
  required_version = ">= 1.7.0"

  required_providers {
    render = {
      source  = "render-oss/render"
      version = "~> 1.8"
    }
    neon = {
      source  = "kislerdm/neon"
      version = "~> 0.18"
    }
    cloudflare = {
      source  = "cloudflare/cloudflare"
      version = "~> 5.0"
    }
  }

  # Remote state config lives in backend.tf (Cloudflare R2).
}

provider "render" {
  api_key  = var.render_api_key
  owner_id = var.render_owner_id
}

provider "neon" {
  api_key = var.neon_api_key
}

provider "cloudflare" {
  api_token = var.cloudflare_api_token
}
