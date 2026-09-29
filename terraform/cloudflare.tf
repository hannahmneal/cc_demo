# DNS + free-tier gateway hardening for cc-demo.redqueen.run, proxied through
# Cloudflare so the API benefits from TLS/WAF/DDoS protection without a Worker.
resource "cloudflare_dns_record" "cc_demo_api" {
  zone_id = var.cloudflare_zone_id
  name    = var.subdomain
  type    = "CNAME"
  content = trimsuffix(trimprefix(render_web_service.cc_demo_api.url, "https://"), "/")
  ttl     = 1 # automatic, required when proxied
  proxied = true
}

# A per-IP rate limiting rule was previously managed here via
# cloudflare_ruleset, but creating a zone-level http_ratelimit ruleset through
# the API consistently 403'd ("Authentication error", code 10000) even with
# Zone WAF:Edit granted on the token - this looks like a Free-plan API/product
# gate rather than a token scope problem (the dashboard's "1 free rate
# limiting rule" allowance appears to be UI-only, not exposed to this API on
# Free). If a rate limit is desired, add it manually in the dashboard:
# Security > WAF > Rate limiting rules.
