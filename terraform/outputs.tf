output "api_url" {
  description = "Public URL for the CC_Demo API once DNS has propagated"
  value       = "https://${var.subdomain}.redqueen.run"
}
