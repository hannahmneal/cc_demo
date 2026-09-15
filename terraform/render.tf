# The API (requirement 2): the only public entry point. Postgres itself has
# no direct public ingress — it's reachable solely via the PG_CONNECTION env
# var wired in below.
#
# NOTE: Render needs credentials to pull a *private* GHCR image (there's a
# render_registry_credential resource for that, plus registry_credential_id
# on runtime_source.image below). Simplest free path: after the first image
# push, open the package on GitHub (Package settings > Change visibility) and
# make ghcr.io/<owner>/cc-demo public, so no credential is needed at all.
resource "render_web_service" "cc_demo_api" {
  name   = "cc-demo-api"
  plan   = "free"
  region = "oregon"

  runtime_source = {
    image = {
      image_url = var.image_repository
      tag       = var.image_tag
    }
  }

  env_vars = {
    "PG_CONNECTION"          = { value = local.pg_connection_string }
    "ASPNETCORE_ENVIRONMENT" = { value = "Production" }
  }

  custom_domains = [
    { name = "${var.subdomain}.redqueen.run" },
  ]
}

output "render_service_hostname" {
  value = render_web_service.cc_demo_api.url
}
